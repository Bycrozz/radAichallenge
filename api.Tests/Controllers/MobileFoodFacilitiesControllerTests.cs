using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

// All API types (controllers, DbContext, models) live in the global namespace
// since no `namespace` declaration is present in the API source.
namespace api.Tests.Controllers
{
    [TestFixture]
    public class MobileFoodTrucksControllerTests
    {
        private ApplicationDbContext _ctx;

        [SetUp]
        public void SetUp()
        {
            // 1) Configure a new in-memory database for each test to ensure total isolation
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            // 2) Create DbContext with the in-memory provider and seed initial data
            _ctx = new ApplicationDbContext(options);
            _ctx.MobileFoodTrucks.Add(new MobileFoodTruck
            {
                Applicant = "Taco Truck",
                Status    = "APPROVED",
                Address   = "1 A St",
                Latitude  = 37.8,
                Longitude = -122.4
            });
            // Save seeded data so it's available during test
            _ctx.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose context and clear in-memory database
            _ctx.Dispose();
        }

        // =====================
        // Tests for searchByName
        // =====================

        [Test]
        public void SearchByName_ExistingNameAndStatus_ReturnsOkObjectResult()
        {
            // Arrange: instantiate controller with seeded context
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: call the action with exact name and status
            var result = controller.SearchByName("Taco", "APPROVED");

            // Assert: verify a 200 OK response containing exactly one matching truck
            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;

            // Extract the returned value and assert its type and contents
            var list = ok.Value.Should()
                .BeAssignableTo<IEnumerable<MobileFoodTruck>>()  // ensure correct return type
                .Which
                .ToList();                                     // convert to List for further assertions

            // Only the seeded "Taco Truck" should match
            list.Should().HaveCount(1)
                .And.OnlyContain(t =>
                    t.Applicant.Contains("Taco", StringComparison.OrdinalIgnoreCase)
                    && t.Status == "APPROVED"
                );
        }

        [Test]
        public void SearchByName_PartialName_NoStatus_ReturnsMatchingTrucks()
        {
            // Arrange: add a second truck containing "Truck" in the name
            _ctx.MobileFoodTrucks.Add(new MobileFoodTruck
            {
                Applicant = "Burger Truck",
                Status    = "REQUESTED",
                Address   = "2 B St",
                Latitude  = 37.7,
                Longitude = -122.3
            });
            _ctx.SaveChanges();
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: search for "truck" without specifying status (null)
            var result = controller.SearchByName("truck", null);

            // Assert: both seeded trucks have "truck" in their name
            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;
            var list = ok.Value.Should()
                .BeAssignableTo<IEnumerable<MobileFoodTruck>>()
                .Which
                .ToList();
            list.Should().HaveCount(2); // "Taco Truck" and "Burger Truck"
        }

        [Test]
        public void SearchByName_EmptyName_ReturnsAllTrucks()
        {
            // Arrange: add a second distinct truck to test empty-name behavior
            _ctx.MobileFoodTrucks.Add(new MobileFoodTruck
            {
                Applicant = "Hotdog Stand",
                Status    = "APPROVED",
                Address   = "3 C St",
                Latitude  = 37.9,
                Longitude = -122.5
            });
            _ctx.SaveChanges();
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: search with an empty name string
            var result = controller.SearchByName("", null);

            // Assert: returns all trucks because code does not filter on empty string
            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;
            var list = ok.Value.Should()
                .BeAssignableTo<IEnumerable<MobileFoodTruck>>()
                .Which
                .ToList();
            list.Should().HaveCount(2); // both "Taco Truck" and "Hotdog Stand"
        }

        [Test]
        public void SearchByName_NoSeededData_ReturnsEmptyList()
        {
            // Arrange: clear any seeded data to simulate an empty database
            _ctx.MobileFoodTrucks.RemoveRange(_ctx.MobileFoodTrucks);
            _ctx.SaveChanges();
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: search in an empty database
            var result = controller.SearchByName("Anything", null);

            // Assert: results list should be empty
            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;
            var list = ok.Value.Should()
                .BeAssignableTo<IEnumerable<MobileFoodTruck>>()
                .Which;
            list.Should().BeEmpty();
        }

        [Test]
        public void SearchByName_IgnoresNullApplicant()
        {
            // Arrange: add a truck with null Applicant to verify null-safety
            _ctx.MobileFoodTrucks.Add(new MobileFoodTruck
            {
                Applicant = null,
                Status    = "APPROVED",
                Address   = "Null St",
                Latitude  = 37.8,
                Longitude = -122.4
            });
            _ctx.SaveChanges();
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: search with empty name so everything is returned except null Applicants
            var result = controller.SearchByName("", null);

            // Assert: only non-null Applicant trucks appear
            var list = ((OkObjectResult)result).Value
                .As<IEnumerable<MobileFoodTruck>>()
                .ToList();
            list.Should().NotContain(t => t.Applicant == null);
        }

        [Test]
        public void SearchByName_InvalidStatus_ReturnsEmptyList()
        {
            // Arrange: use a status that does not match any truck
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: search with an invalid status
            var result = controller.SearchByName("Taco", "INVALID");

            // Assert: no trucks should be returned
            var list = ((OkObjectResult)result).Value
                .As<IEnumerable<MobileFoodTruck>>();
            list.Should().BeEmpty();
        }

        // =======================
        // Tests for searchByStreet
        // =======================

        [Test]
        public void SearchByStreet_MatchingStreet_ReturnsOkWithTrucks()
        {
            // Arrange: add another truck on a different street for coverage
            _ctx.MobileFoodTrucks.Add(new MobileFoodTruck
            {
                Applicant = "Sushi Truck",
                Status    = "APPROVED",
                Address   = "Main St",
                Latitude  = 37.8,
                Longitude = -122.4
            });
            _ctx.SaveChanges();
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: search by substring of the Address field
            var result = controller.SearchByStreet("A St");

            // Assert: only addresses containing "A St" are returned
            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;
            var list = ok.Value.Should()
                .BeAssignableTo<IEnumerable<MobileFoodTruck>>()
                .Which;
            list.Should().OnlyContain(t =>
                t.Address.Contains("A St", StringComparison.OrdinalIgnoreCase)
            );
        }

        [Test]
        public void SearchByStreet_NoMatchingStreet_ReturnsEmptyList()
        {
            // Arrange
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: search with a street name not in any Address
            var result = controller.SearchByStreet("Nonexistent");

            // Assert: empty list of results
            var ok = result.Should().BeOfType<OkObjectResult>().Subject as OkObjectResult;
            var list = (IEnumerable<MobileFoodTruck>)ok.Value;
            list.Should().BeEmpty();
        }

        [Test]
        public void SearchByStreet_NoSeededData_ReturnsEmptyList()
        {
            // Arrange: clear all data
            _ctx.MobileFoodTrucks.RemoveRange(_ctx.MobileFoodTrucks);
            _ctx.SaveChanges();
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: search on empty DB
            var result = controller.SearchByStreet("Any");

            // Assert: still returns empty list
            var list = ((OkObjectResult)result).Value
                .As<IEnumerable<MobileFoodTruck>>();
            list.Should().BeEmpty();
        }

        [Test]
        public void SearchByStreet_IgnoresNullAddress()
        {
            // Arrange: add truck with null Address to check null-safety
            _ctx.MobileFoodTrucks.Add(new MobileFoodTruck
            {
                Applicant = "NullAddr Truck",
                Status    = "APPROVED",
                Address   = null,
                Latitude  = 37.8,
                Longitude = -122.4
            });
            _ctx.SaveChanges();
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: search by empty substring (will return non-null addresses)
            var result = controller.SearchByStreet("");

            // Assert: no exception, and trucks with non-null Address appear
            var list = ((OkObjectResult)result).Value
                .As<IEnumerable<MobileFoodTruck>>();
            list.Should().NotContain(t => t.Address == null);
        }

        // =============================
        // Tests for nearestFoodTrucks
        // =============================

        [Test]
        public void NearestFoodTrucks_DefaultStatus_ReturnsClosestApproved()
        {
            // Arrange: seed additional approved trucks at varying distances
            _ctx.MobileFoodTrucks.AddRange(new[]
            {
                new MobileFoodTruck
                {
                    Applicant = "Close Truck", Status = "APPROVED",
                    Address = "Close St", Latitude = 37.8001, Longitude = -122.4001
                },
                new MobileFoodTruck
                {
                    Applicant = "Far Truck", Status = "APPROVED",
                    Address = "Far St", Latitude = 40.0, Longitude = -120.0
                }
            });
            _ctx.SaveChanges();
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: request nearest to a specific lat/long
            var result = controller.NearestFoodTrucks(37.8, -122.4);

            // Assert: results sorted by distance, top two are Taco then Close
            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;
            var list = ok.Value.Should()
                .BeAssignableTo<IEnumerable<MobileFoodTruck>>()
                .Which
                .ToList();
            list.Select(t => t.Applicant)
                .Should().ContainInOrder("Taco Truck", "Close Truck");
        }

        [Test]
        public void NearestFoodTrucks_CustomStatus_ReturnsOnlyRequested()
        {
            // Arrange: seed a requested-status truck very close to origin
            _ctx.MobileFoodTrucks.Add(new MobileFoodTruck
            {
                Applicant = "Requested Truck", Status = "REQUESTED",
                Address = "Nearby St", Latitude = 37.8002, Longitude = -122.4002
            });
            _ctx.SaveChanges();
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: call with status filter
            var result = controller.NearestFoodTrucks(37.8, -122.4, "REQUESTED");

            // Assert: only the requested truck appears
            var list = ((OkObjectResult)result).Value
                .As<IEnumerable<MobileFoodTruck>>()
                .ToList();
            list.Should().HaveCount(1)
                .And.OnlyContain(t => t.Status == "REQUESTED");
        }

        [Test]
        public void NearestFoodTrucks_NoSeededData_ReturnsEmptyList()
        {
            // Arrange: clear DB for empty scenario
            _ctx.MobileFoodTrucks.RemoveRange(_ctx.MobileFoodTrucks);
            _ctx.SaveChanges();
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: call on empty data
            var result = controller.NearestFoodTrucks(37.8, -122.4, null);

            // Assert: empty results
            var list = ((OkObjectResult)result).Value
                .As<IEnumerable<MobileFoodTruck>>();
            list.Should().BeEmpty();
        }

        [Test]
        public void NearestFoodTrucks_EnforcesTakeFiveLimit()
        {
            // Arrange: seed more than five approved trucks to test the Take(5) cap
            for (int i = 0; i < 10; i++)
            {
                _ctx.MobileFoodTrucks.Add(new MobileFoodTruck
                {
                    Applicant = $"T{i}",
                    Status    = "APPROVED",
                    Address   = "St",
                    Latitude  = 37.8 + i * 0.001,
                    Longitude = -122.4
                });
            }
            _ctx.SaveChanges();
            var controller = new MobileFoodTrucksController(_ctx);

            // Act: request nearest (should apply Take(5) inside the method)
            var list = ((OkObjectResult)controller
                .NearestFoodTrucks(37.8, -122.4))
                .Value
                .As<IEnumerable<MobileFoodTruck>>()
                .ToList();

            // Assert: no more than 5 items returned
            list.Should().HaveCount(5);
        }

        [Test]
        public void NearestFoodTrucks_EmptyStatus_TreatedAsDefault()
        {
            // Arrange: compare calls with empty status and default null status
            var controller = new MobileFoodTrucksController(_ctx);

            // Act
            var listEmptyStatus = ((OkObjectResult)controller
                .NearestFoodTrucks(37.8, -122.4, ""))
                .Value
                .As<IEnumerable<MobileFoodTruck>>()
                .ToList();
            var listDefault = ((OkObjectResult)controller
                .NearestFoodTrucks(37.8, -122.4))
                .Value
                .As<IEnumerable<MobileFoodTruck>>()
                .ToList();

            // Assert: empty string status behaves the same as default ("APPROVED") filter
            listEmptyStatus.Should().BeEquivalentTo(listDefault);
        }

        [Test]
        public void NearestFoodTrucks_ZeroCoordinates_WorksWithoutException()
        {
            // Arrange & Act: calling with (0,0) should not throw
            Action act = () => new MobileFoodTrucksController(_ctx)
                .NearestFoodTrucks(0, 0, null);

            // Assert: method executes without exceptions
            act.Should().NotThrow();
        }
    }
}

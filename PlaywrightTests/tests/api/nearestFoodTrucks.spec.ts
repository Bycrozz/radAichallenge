import { test, expect } from '@playwright/test';

const base = '/api/MobileFoodTrucks';

test.describe('GET /nearestFoodTrucks', () => {
  test('should return up to 5 closest APPROVED trucks by default', async ({ request }) => {
    const res = await request.get(`${base}/nearestFoodTrucks?latitude=37.8&longitude=-122.4`);
    expect(res.status()).toBe(200);
    const json = await res.json();
    expect(Array.isArray(json)).toBe(true);
    expect(json.length).toBeLessThanOrEqual(5);
    json.forEach(t => expect(t.status).toBe('APPROVED'));
  });

  test('should return REQUESTED trucks when status=REQUESTED', async ({ request }) => {
    const res = await request.get(`${base}/nearestFoodTrucks?latitude=37.8&longitude=-122.4&status=REQUESTED`);
    expect(res.status()).toBe(200);
    const json = await res.json();
    json.forEach(t => expect(t.status).toBe('REQUESTED'));
  });

  test('should return 400 if latitude is missing', async ({ request }) => {
    const res = await request.get(`${base}/nearestFoodTrucks?longitude=-122.4`);
    expect(res.status()).toBe(400);
  });

  test('should return 400 if longitude is missing', async ({ request }) => {
    const res = await request.get(`${base}/nearestFoodTrucks?latitude=37.8`);
    expect(res.status()).toBe(400);
  });

  test('should return empty array if no trucks match status', async ({ request }) => {
    const res = await request.get(`${base}/nearestFoodTrucks?latitude=37.8&longitude=-122.4&status=NONEXISTENT`);
    expect(res.status()).toBe(200);
    const json = await res.json();
    expect(json).toEqual([]);
  });

  test('should return 200 and an empty array for far west coordinates (Pacific Ocean)', async ({ request }) => {
    const res = await request.get(`${base}?latitude=0&longitude=-150`);
    expect(res.status()).toBe(200);

    const json = await res.json();
    expect(Array.isArray(json)).toBe(true);
    expect(json.length).toBe(0);
  });

  test('should return 200 and an empty array for far north coordinates (Alaska)', async ({ request }) => {
    const res = await request.get(`${base}?latitude=65&longitude=-150`);
    expect(res.status()).toBe(200);

    const json = await res.json();
    expect(Array.isArray(json)).toBe(true);
    expect(json.length).toBe(0);
  });

  test('should return 200 and an empty array for east-coast coordinates (New York)', async ({ request }) => {
    const res = await request.get(`${base}?latitude=40.7128&longitude=-74.006`);
    expect(res.status()).toBe(200);

    const json = await res.json();
    expect(Array.isArray(json)).toBe(true);
    expect(json.length).toBe(0);
  });
});

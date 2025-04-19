import { test, expect } from '@playwright/test';

const base = '/api/MobileFoodTrucks';
const statuses = ['APPROVED', 'REQUESTED', 'EXPIRED', 'SUSPENDED'];

test.describe('GET /searchByName', () => {
  test('should return 400 when name is missing', async ({ request }) => {
    const res = await request.get(`${base}/searchByName`);
    expect(res.status()).toBe(400);
  });

  test('should return 200 and list when name matches', async ({ request }) => {
    const res = await request.get(`${base}/searchByName?name=Taco`);
    expect(res.status()).toBe(200);
    const json = await res.json();
    expect(Array.isArray(json)).toBe(true);
    expect(json.length).toBeGreaterThan(0);
    const hasTaco = json.some((item: any) =>
      typeof item.applicant === 'string' &&
      item.applicant.toLowerCase().includes('taco')
    );
    expect(hasTaco).toBe(true);
  });

  for (const status of statuses) {
    test(`should return only ${status} trucks when name and status=${status}`, async ({ request }) => {
      const res = await request.get(`${base}/searchByName?name=Taco&status=${status}`);
      expect(res.status()).toBe(200);
      const json = await res.json();
      expect(Array.isArray(json)).toBe(true);
      json.forEach((t: any) => {
        expect(t.status).toBe(status);
      });
    });
  }

  test('should return empty array when no matches found', async ({ request }) => {
    const res = await request.get(`${base}/searchByName?name=NoSuchTruck`);
    expect(res.status()).toBe(200);
    const json = await res.json();
    expect(json).toEqual([]);
  });
});

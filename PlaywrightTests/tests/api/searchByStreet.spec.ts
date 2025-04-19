import { test, expect } from '@playwright/test';

const base = '/api/MobileFoodTrucks';

test.describe('GET /searchByStreet', () => {
  test('should return 200 and trucks for a valid street', async ({ request }) => {
    const res = await request.get(`${base}/searchByStreet?street=A St`);
    expect(res.status()).toBe(200);
    const json = await res.json();
    expect(Array.isArray(json)).toBe(true);
    expect(json.length).toBeGreaterThan(0);
    json.forEach(truck => {
      expect(truck.address).toMatch(/A St/i);
    });
  });

  test('should return empty array when street has no trucks', async ({ request }) => {
    const res = await request.get(`${base}/searchByStreet?street=Nowhere`);
    expect(res.status()).toBe(200);
    const json = await res.json();
    expect(json).toEqual([]);
  });

  test('should return 400 if street param is missing', async ({ request }) => {
    const res = await request.get(`${base}/searchByStreet`);
    expect(res.status()).toBe(400);
  });
});

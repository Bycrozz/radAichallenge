import { test, expect } from '@playwright/test';

test.describe('Food Truck Search UI', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:5000/foodTruckSearch/index.html');
  });

  test('should load the page with key UI elements [smoke][ui]', async ({ page }) => {
    await expect(page.locator('text=Food Truck Search')).toBeVisible();
    await expect(page.locator('#searchBy')).toBeVisible();
    await expect(page.locator('#searchInput')).toBeVisible();
    await expect(page.locator('button:has-text("Search")')).toBeVisible();
    await expect(page.locator('button:has-text("Clear")')).toBeVisible();
  });

  test('should show validation message when no input is provided [validation][ui]', async ({ page }) => {
    await page.locator('button:has-text("Search")').click();
    await expect(page.locator('#errorBox')).toBeVisible();
    await expect(page.locator('#errorMsg')).toContainText('Please enter');
  });

  test('should return results for a valid name search [search][name][positive]', async ({ page }) => {
    await page.fill('#searchInput', 'Taco');
    await page.locator('button:has-text("Search")').click();
    await page.waitForSelector('#resultsTable');
    const rows = await page.locator('#resultsBody tr').count();
    expect(rows).toBeGreaterThan(0);
  });

  test('should show error for invalid coordinates in nearest search [search][nearest][negative][validation]', async ({ page }) => {
    await page.selectOption('#searchBy', 'nearest');
    await page.fill('#latInput', '0');
    await page.fill('#lngInput', '0');
    await page.locator('button:has-text("Search")').click();
    await expect(page.locator('#errorBox')).toBeVisible();
    await expect(page.locator('#errorMsg')).toContainText('Latitude and longitude cannot be 0.');
  });

  test('clear button should reset the form and hide results [clear][ui]', async ({ page }) => {
    await page.fill('#searchInput', 'Taco');
    await page.locator('button:has-text("Search")').click();
    await page.waitForSelector('#resultsTable');
    await page.locator('#clearBtn').click();
    await expect(page.locator('#resultsTable')).toBeHidden();
    await expect(page.locator('#searchInput')).toHaveValue('');
  });

  test('should show paginated results if more than 15 found [pagination][search]', async ({ page }) => {
    await page.fill('#searchInput', 'Pizza');
    await page.locator('button:has-text("Search")').click();
    await page.waitForSelector('#resultsTable');

    const rows = await page.locator('#resultsBody tr').count();
    expect(rows).toBeGreaterThan(0);

    const paginationButtons = await page.locator('#paginationContainer button').count();
    expect(paginationButtons).toBeGreaterThan(0);
  });

  test('should display street placeholder when street is selected [dropdown][ui]', async ({ page }) => {
    await page.selectOption('#searchBy', 'street');
    await expect(page.locator('#searchInput')).toHaveAttribute('placeholder', 'Type address');
  });

  test('should display latitude and longitude inputs when nearest is selected [dropdown][ui][nearest]', async ({ page }) => {
    await page.selectOption('#searchBy', 'nearest');
    await expect(page.locator('#latInput')).toBeVisible();
    await expect(page.locator('#lngInput')).toBeVisible();
    await expect(page.locator('#searchInput')).toBeHidden();
  });

  test('should hide error box when dismissed [error][ui]', async ({ page }) => {
    await page.locator('button:has-text("Search")').click();
    await expect(page.locator('#errorBox')).toBeVisible();
    await page.click('#errorBox button');
    await expect(page.locator('#errorBox')).toBeHidden();
  });
});

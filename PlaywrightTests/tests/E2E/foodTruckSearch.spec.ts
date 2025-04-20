import { test, expect } from '@playwright/test';

test.describe('@E2E Food Truck Search UI', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:5000/foodTruckSearch/index.html');
  });

  test('@smoke @ui should load the page with key UI elements', async ({ page }) => {
    await expect(page.locator('text=Food Truck Search')).toBeVisible();
    await expect(page.locator('#searchBy')).toBeVisible();
    await expect(page.locator('#searchInput')).toBeVisible();
    await expect(page.locator('button:has-text("Search")')).toBeVisible();
    await expect(page.locator('button:has-text("Clear")')).toBeVisible();
  });

  test('@validation @ui should show validation message when no input is provided', async ({ page }) => {
    await page.locator('button:has-text("Search")').click();
    await expect(page.locator('#errorBox')).toBeVisible();
    await expect(page.locator('#errorMsg')).toContainText('Please enter');
  });

  test('@search @name @positive should return results for a valid name search', async ({ page }) => {
    await page.fill('#searchInput', 'Taco');
    await page.locator('button:has-text("Search")').click();
    await page.waitForSelector('#resultsTable');
    const rows = await page.locator('#resultsBody tr').count();
    expect(rows).toBeGreaterThan(0);
  });

  test('@search @nearest @negative @validation should show error for invalid coordinates in nearest search', async ({ page }) => {
    await page.selectOption('#searchBy', 'nearest');
    await page.fill('#latInput', '0');
    await page.fill('#lngInput', '0');
    await page.locator('button:has-text("Search")').click();
    await expect(page.locator('#errorBox')).toBeVisible();
    await expect(page.locator('#errorMsg')).toContainText('Latitude and longitude cannot be 0.');
  });

  test('@clear @ui clear button should reset the form and hide results', async ({ page }) => {
    await page.fill('#searchInput', 'Taco');
    await page.locator('button:has-text("Search")').click();
    await page.waitForSelector('#resultsTable');
    await page.locator('#clearBtn').click();
    await expect(page.locator('#resultsTable')).toBeHidden();
    await expect(page.locator('#searchInput')).toHaveValue('');
  });

  test('@pagination @search should show paginated results if more than 15 found', async ({ page }) => {
    await page.fill('#searchInput', 'Pizza');
    await page.locator('button:has-text("Search")').click();
    await page.waitForSelector('#resultsTable');

    const rows = await page.locator('#resultsBody tr').count();
    expect(rows).toBeGreaterThan(0);

    const paginationButtons = await page.locator('#paginationContainer button').count();
    expect(paginationButtons).toBeGreaterThan(0);
  });

  test('@dropdown @ui should display street placeholder when street is selected', async ({ page }) => {
    await page.selectOption('#searchBy', 'street');
    await expect(page.locator('#searchInput')).toHaveAttribute('placeholder', 'Type address');
  });

  test('@dropdown @ui @nearest should display latitude and longitude inputs when nearest is selected', async ({ page }) => {
    await page.selectOption('#searchBy', 'nearest');
    await expect(page.locator('#latInput')).toBeVisible();
    await expect(page.locator('#lngInput')).toBeVisible();
    await expect(page.locator('#searchInput')).toBeHidden();
  });

  test('@error @ui should hide error box when dismissed', async ({ page }) => {
    await page.locator('button:has-text("Search")').click();
    await expect(page.locator('#errorBox')).toBeVisible();
    await page.click('#errorBox button');
    await expect(page.locator('#errorBox')).toBeHidden();
  });
});

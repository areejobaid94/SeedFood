import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

export interface Product {
  productId: string;
  name: string;
  retailer_Id: string;
  description: string;
  price: string;
  currency: string;
  imageUrl: string;
}

export interface Catalog {
  catalogId: string;
  catalogName: string;
  products: Product[];
}

@Injectable({
  providedIn: 'root'
})
export class CatalogService {

  constructor() { }

  // Fake catalog data as if coming from WhatsApp API
  private catalog: Catalog = {
    catalogId: '1234567890',
    catalogName: 'My Shop Catalog',
    products: [
      { productId: 'p1', name: 'Wireless Headphones',retailer_Id:'1', description: 'Bluetooth headphones with noise cancellation', price: '50.00', currency: 'USD', imageUrl: 'https://example.com/images/headphones.jpg' },
      { productId: 'p2', name: 'Smart Watch',retailer_Id:'2', description: 'Waterproof smart watch with heart rate monitor', price: '80.00', currency: 'USD', imageUrl: 'https://example.com/images/watch.jpg' },
      { productId: 'p3', name: 'Gaming Mouse',retailer_Id:'3', description: 'High precision wireless gaming mouse', price: '25.00', currency: 'USD', imageUrl: 'https://example.com/images/mouse.jpg' }
    ]
  };

  // Method to get the catalog
  getCatalog(): Observable<Catalog> {
    return of(this.catalog);
  }

  // Method to get single product by ID
  getProductById(productId: string): Observable<Product | undefined> {
    const product = this.catalog.products.find(p => p.productId === productId);
    return of(product);
  }
}

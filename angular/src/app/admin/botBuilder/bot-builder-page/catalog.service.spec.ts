/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { CatalogService } from './catalog.service';

describe('Service: Catalog', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CatalogService]
    });
  });

  it('should ...', inject([CatalogService], (service: CatalogService) => {
    expect(service).toBeTruthy();
  }));
});

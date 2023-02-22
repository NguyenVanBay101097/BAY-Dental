import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductManagementProductsComponent } from './product-management-products.component';

describe('ProductManagementProductsComponent', () => {
  let component: ProductManagementProductsComponent;
  let fixture: ComponentFixture<ProductManagementProductsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductManagementProductsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductManagementProductsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

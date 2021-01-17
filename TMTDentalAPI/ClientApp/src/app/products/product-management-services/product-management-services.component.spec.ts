import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductManagementServicesComponent } from './product-management-services.component';

describe('ProductManagementServicesComponent', () => {
  let component: ProductManagementServicesComponent;
  let fixture: ComponentFixture<ProductManagementServicesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductManagementServicesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductManagementServicesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

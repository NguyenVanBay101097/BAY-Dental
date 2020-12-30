import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductManagementMedicinesComponent } from './product-management-medicines.component';

describe('ProductManagementMedicinesComponent', () => {
  let component: ProductManagementMedicinesComponent;
  let fixture: ComponentFixture<ProductManagementMedicinesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductManagementMedicinesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductManagementMedicinesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

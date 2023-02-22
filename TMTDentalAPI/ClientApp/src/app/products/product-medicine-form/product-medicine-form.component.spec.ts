import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductMedicineFormComponent } from './product-medicine-form.component';

describe('ProductMedicineFormComponent', () => {
  let component: ProductMedicineFormComponent;
  let fixture: ComponentFixture<ProductMedicineFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductMedicineFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductMedicineFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

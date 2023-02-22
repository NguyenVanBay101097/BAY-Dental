import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductMedicineListComponent } from './product-medicine-list.component';

describe('ProductMedicineListComponent', () => {
  let component: ProductMedicineListComponent;
  let fixture: ComponentFixture<ProductMedicineListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductMedicineListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductMedicineListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

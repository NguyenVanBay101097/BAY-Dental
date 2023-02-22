import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicineOrderPrescriptionPaymentListComponent } from './medicine-order-prescription-payment-list.component';

describe('MedicineOrderPrescriptionPaymentListComponent', () => {
  let component: MedicineOrderPrescriptionPaymentListComponent;
  let fixture: ComponentFixture<MedicineOrderPrescriptionPaymentListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MedicineOrderPrescriptionPaymentListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicineOrderPrescriptionPaymentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

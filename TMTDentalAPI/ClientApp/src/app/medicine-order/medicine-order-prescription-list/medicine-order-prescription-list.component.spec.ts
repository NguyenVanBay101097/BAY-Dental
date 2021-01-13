import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicineOrderPrescriptionListComponent } from './medicine-order-prescription-list.component';

describe('MedicineOrderPrescriptionListComponent', () => {
  let component: MedicineOrderPrescriptionListComponent;
  let fixture: ComponentFixture<MedicineOrderPrescriptionListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MedicineOrderPrescriptionListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicineOrderPrescriptionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

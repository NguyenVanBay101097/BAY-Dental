import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerSupplierFormDebitComponent } from './partner-supplier-form-debit.component';

describe('PartnerSupplierFormDebitComponent', () => {
  let component: PartnerSupplierFormDebitComponent;
  let fixture: ComponentFixture<PartnerSupplierFormDebitComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerSupplierFormDebitComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerSupplierFormDebitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

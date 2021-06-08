import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerDebtManagementComponent } from './partner-customer-debt-management.component';

describe('PartnerCustomerDebtManagementComponent', () => {
  let component: PartnerCustomerDebtManagementComponent;
  let fixture: ComponentFixture<PartnerCustomerDebtManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerDebtManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerDebtManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

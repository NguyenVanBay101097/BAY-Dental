import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerInfoCustomerManagementComponent } from './partner-info-customer-management.component';

describe('PartnerInfoCustomerManagementComponent', () => {
  let component: PartnerInfoCustomerManagementComponent;
  let fixture: ComponentFixture<PartnerInfoCustomerManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerInfoCustomerManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerInfoCustomerManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

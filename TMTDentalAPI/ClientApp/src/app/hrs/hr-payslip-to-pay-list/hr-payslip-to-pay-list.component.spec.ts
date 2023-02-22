import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrPayslipToPayListComponent } from './hr-payslip-to-pay-list.component';

describe('HrPayslipToPayListComponent', () => {
  let component: HrPayslipToPayListComponent;
  let fixture: ComponentFixture<HrPayslipToPayListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrPayslipToPayListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrPayslipToPayListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

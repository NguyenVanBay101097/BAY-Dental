import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrPayslipToPayCreateUpdateComponent } from './hr-payslip-to-pay-create-update.component';

describe('HrPayslipToPayCreateUpdateComponent', () => {
  let component: HrPayslipToPayCreateUpdateComponent;
  let fixture: ComponentFixture<HrPayslipToPayCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrPayslipToPayCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrPayslipToPayCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

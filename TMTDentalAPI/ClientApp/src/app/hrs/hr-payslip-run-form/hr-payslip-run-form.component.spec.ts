import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrPayslipRunFormComponent } from './hr-payslip-run-form.component';

describe('HrPayslipRunFormComponent', () => {
  let component: HrPayslipRunFormComponent;
  let fixture: ComponentFixture<HrPayslipRunFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrPayslipRunFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrPayslipRunFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

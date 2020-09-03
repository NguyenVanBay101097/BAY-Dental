import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrPayslipRunListComponent } from './hr-payslip-run-list.component';

describe('HrPayslipRunListComponent', () => {
  let component: HrPayslipRunListComponent;
  let fixture: ComponentFixture<HrPayslipRunListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrPayslipRunListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrPayslipRunListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

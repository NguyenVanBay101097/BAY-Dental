import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrPayslipDateFilterComponent } from './hr-payslip-date-filter.component';

describe('HrPayslipDateFilterComponent', () => {
  let component: HrPayslipDateFilterComponent;
  let fixture: ComponentFixture<HrPayslipDateFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrPayslipDateFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrPayslipDateFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

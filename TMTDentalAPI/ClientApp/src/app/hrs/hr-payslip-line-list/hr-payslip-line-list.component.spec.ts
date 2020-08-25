import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrPayslipLineListComponent } from './hr-payslip-line-list.component';

describe('HrPayslipLineListComponent', () => {
  let component: HrPayslipLineListComponent;
  let fixture: ComponentFixture<HrPayslipLineListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrPayslipLineListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrPayslipLineListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardCustomerReceiptListTodayComponent } from './dashboard-customer-receipt-list-today.component';

describe('DashboardCustomerReceiptListTodayComponent', () => {
  let component: DashboardCustomerReceiptListTodayComponent;
  let fixture: ComponentFixture<DashboardCustomerReceiptListTodayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardCustomerReceiptListTodayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardCustomerReceiptListTodayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountPaymentListTComponent } from './account-payment-list-t.component';

describe('AccountPaymentListTComponent', () => {
  let component: AccountPaymentListTComponent;
  let fixture: ComponentFixture<AccountPaymentListTComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountPaymentListTComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountPaymentListTComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountPaymentListComponent } from './account-payment-list.component';

describe('AccountPaymentListComponent', () => {
  let component: AccountPaymentListComponent;
  let fixture: ComponentFixture<AccountPaymentListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountPaymentListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountPaymentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

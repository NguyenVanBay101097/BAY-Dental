import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountPaymentCreateUpdateComponent } from './account-payment-create-update.component';

describe('AccountPaymentCreateUpdateComponent', () => {
  let component: AccountPaymentCreateUpdateComponent;
  let fixture: ComponentFixture<AccountPaymentCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountPaymentCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountPaymentCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

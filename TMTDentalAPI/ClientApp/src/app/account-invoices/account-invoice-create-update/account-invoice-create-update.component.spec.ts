import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceCreateUpdateComponent } from './account-invoice-create-update.component';

describe('AccountInvoiceCreateUpdateComponent', () => {
  let component: AccountInvoiceCreateUpdateComponent;
  let fixture: ComponentFixture<AccountInvoiceCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceAdvanceSearchComponent } from './account-invoice-advance-search.component';

describe('AccountInvoiceAdvanceSearchComponent', () => {
  let component: AccountInvoiceAdvanceSearchComponent;
  let fixture: ComponentFixture<AccountInvoiceAdvanceSearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceAdvanceSearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceAdvanceSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

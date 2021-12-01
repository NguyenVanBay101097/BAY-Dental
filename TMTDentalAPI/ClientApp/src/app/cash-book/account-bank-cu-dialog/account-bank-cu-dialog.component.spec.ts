import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountBankCuDialogComponent } from './account-bank-cu-dialog.component';

describe('AccountBankCuDialogComponent', () => {
  let component: AccountBankCuDialogComponent;
  let fixture: ComponentFixture<AccountBankCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountBankCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountBankCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

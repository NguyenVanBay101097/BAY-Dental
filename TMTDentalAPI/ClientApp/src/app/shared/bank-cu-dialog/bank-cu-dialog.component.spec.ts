import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BankCuDialogComponent } from './bank-cu-dialog.component';

describe('BankCuDialogComponent', () => {
  let component: BankCuDialogComponent;
  let fixture: ComponentFixture<BankCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BankCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BankCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

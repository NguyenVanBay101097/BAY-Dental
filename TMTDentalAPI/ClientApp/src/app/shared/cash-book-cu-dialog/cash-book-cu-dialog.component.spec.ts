import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CashBookCuDialogComponent } from './cash-book-cu-dialog.component';

describe('CashBookCuDialogComponent', () => {
  let component: CashBookCuDialogComponent;
  let fixture: ComponentFixture<CashBookCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CashBookCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CashBookCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

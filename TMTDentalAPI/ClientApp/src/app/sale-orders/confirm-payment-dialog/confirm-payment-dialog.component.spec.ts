import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmPaymentDialogComponent } from './confirm-payment-dialog.component';

describe('ConfirmPaymentDialogComponent', () => {
  let component: ConfirmPaymentDialogComponent;
  let fixture: ComponentFixture<ConfirmPaymentDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfirmPaymentDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmPaymentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

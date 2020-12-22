import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderReceiptDialogComponent } from './labo-order-receipt-dialog.component';

describe('LaboOrderReceiptDialogComponent', () => {
  let component: LaboOrderReceiptDialogComponent;
  let fixture: ComponentFixture<LaboOrderReceiptDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderReceiptDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderReceiptDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

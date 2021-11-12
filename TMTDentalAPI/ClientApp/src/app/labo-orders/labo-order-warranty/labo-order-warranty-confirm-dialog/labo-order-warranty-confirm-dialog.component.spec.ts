import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderWarrantyConfirmDialogComponent } from './labo-order-warranty-confirm-dialog.component';

describe('LaboOrderWarrantyConfirmDialogComponent', () => {
  let component: LaboOrderWarrantyConfirmDialogComponent;
  let fixture: ComponentFixture<LaboOrderWarrantyConfirmDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderWarrantyConfirmDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderWarrantyConfirmDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

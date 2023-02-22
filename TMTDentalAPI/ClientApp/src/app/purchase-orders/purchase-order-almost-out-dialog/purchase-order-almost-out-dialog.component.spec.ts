import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseOrderAlmostOutDialogComponent } from './purchase-order-almost-out-dialog.component';

describe('PurchaseOrderAlmostOutDialogComponent', () => {
  let component: PurchaseOrderAlmostOutDialogComponent;
  let fixture: ComponentFixture<PurchaseOrderAlmostOutDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseOrderAlmostOutDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseOrderAlmostOutDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

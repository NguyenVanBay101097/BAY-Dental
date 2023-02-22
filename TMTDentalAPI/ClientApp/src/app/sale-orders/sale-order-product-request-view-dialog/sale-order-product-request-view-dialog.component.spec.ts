import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderProductRequestViewDialogComponent } from './sale-order-product-request-view-dialog.component';

describe('SaleOrderProductRequestViewDialogComponent', () => {
  let component: SaleOrderProductRequestViewDialogComponent;
  let fixture: ComponentFixture<SaleOrderProductRequestViewDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderProductRequestViewDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderProductRequestViewDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

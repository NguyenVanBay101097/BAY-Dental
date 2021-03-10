import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderProductRequestDialogComponent } from './sale-order-product-request-dialog.component';

describe('SaleOrderProductRequestDialogComponent', () => {
  let component: SaleOrderProductRequestDialogComponent;
  let fixture: ComponentFixture<SaleOrderProductRequestDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderProductRequestDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderProductRequestDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

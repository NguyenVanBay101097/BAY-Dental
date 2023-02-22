import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderCuDialogComponent } from './sale-order-cu-dialog.component';

describe('SaleOrderCuDialogComponent', () => {
  let component: SaleOrderCuDialogComponent;
  let fixture: ComponentFixture<SaleOrderCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

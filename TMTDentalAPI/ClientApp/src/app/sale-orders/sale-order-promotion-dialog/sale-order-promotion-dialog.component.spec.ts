import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderPromotionDialogComponent } from './sale-order-promotion-dialog.component';

describe('SaleOrderPromotionDialogComponent', () => {
  let component: SaleOrderPromotionDialogComponent;
  let fixture: ComponentFixture<SaleOrderPromotionDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderPromotionDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderPromotionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

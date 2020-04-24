import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderApplyServiceCardsDialogComponent } from './sale-order-apply-service-cards-dialog.component';

describe('SaleOrderApplyServiceCardsDialogComponent', () => {
  let component: SaleOrderApplyServiceCardsDialogComponent;
  let fixture: ComponentFixture<SaleOrderApplyServiceCardsDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderApplyServiceCardsDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderApplyServiceCardsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

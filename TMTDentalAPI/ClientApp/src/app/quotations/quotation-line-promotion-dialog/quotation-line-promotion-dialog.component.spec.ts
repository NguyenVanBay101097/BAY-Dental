import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QuotationLinePromotionDialogComponent } from './quotation-line-promotion-dialog.component';

describe('QuotationLinePromotionDialogComponent', () => {
  let component: QuotationLinePromotionDialogComponent;
  let fixture: ComponentFixture<QuotationLinePromotionDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QuotationLinePromotionDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QuotationLinePromotionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

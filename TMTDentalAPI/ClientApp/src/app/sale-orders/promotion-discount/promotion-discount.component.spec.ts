import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PromotionDiscountComponent } from './promotion-discount.component';

describe('PromotionDiscountComponent', () => {
  let component: PromotionDiscountComponent;
  let fixture: ComponentFixture<PromotionDiscountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PromotionDiscountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PromotionDiscountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

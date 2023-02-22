import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOverviewPromotionComponent } from './partner-overview-promotion.component';

describe('PartnerOverviewPromotionComponent', () => {
  let component: PartnerOverviewPromotionComponent;
  let fixture: ComponentFixture<PartnerOverviewPromotionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOverviewPromotionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOverviewPromotionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

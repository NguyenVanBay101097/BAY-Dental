import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SalePromotionProgramCreateUpdateComponent } from './sale-promotion-program-create-update.component';

describe('SalePromotionProgramCreateUpdateComponent', () => {
  let component: SalePromotionProgramCreateUpdateComponent;
  let fixture: ComponentFixture<SalePromotionProgramCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SalePromotionProgramCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SalePromotionProgramCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

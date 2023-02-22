import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SalePromotionProgramListComponent } from './sale-promotion-program-list.component';

describe('SalePromotionProgramListComponent', () => {
  let component: SalePromotionProgramListComponent;
  let fixture: ComponentFixture<SalePromotionProgramListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SalePromotionProgramListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SalePromotionProgramListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

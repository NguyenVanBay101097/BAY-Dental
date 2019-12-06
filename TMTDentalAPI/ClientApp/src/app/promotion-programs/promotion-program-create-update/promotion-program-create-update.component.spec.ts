import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PromotionProgramCreateUpdateComponent } from './promotion-program-create-update.component';

describe('PromotionProgramCreateUpdateComponent', () => {
  let component: PromotionProgramCreateUpdateComponent;
  let fixture: ComponentFixture<PromotionProgramCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PromotionProgramCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PromotionProgramCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

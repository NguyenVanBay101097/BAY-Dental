import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PromotionProgramRuleCuDialogComponent } from './promotion-program-rule-cu-dialog.component';

describe('PromotionProgramRuleCuDialogComponent', () => {
  let component: PromotionProgramRuleCuDialogComponent;
  let fixture: ComponentFixture<PromotionProgramRuleCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PromotionProgramRuleCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PromotionProgramRuleCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

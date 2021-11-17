import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardTypeApplyAllDialogComponent } from './card-type-apply-all-dialog.component';

describe('CardTypeApplyAllDialogComponent', () => {
  let component: CardTypeApplyAllDialogComponent;
  let fixture: ComponentFixture<CardTypeApplyAllDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardTypeApplyAllDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardTypeApplyAllDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

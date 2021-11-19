import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardCardsMemberCuDialogComponent } from './card-cards-member-cu-dialog.component';

describe('CardCardsMemberCuDialogComponent', () => {
  let component: CardCardsMemberCuDialogComponent;
  let fixture: ComponentFixture<CardCardsMemberCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardCardsMemberCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardCardsMemberCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

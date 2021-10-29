import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardCardsMemberCreateDialogComponent } from './card-cards-member-create-dialog.component';

describe('CardCardsMemberCreateDialogComponent', () => {
  let component: CardCardsMemberCreateDialogComponent;
  let fixture: ComponentFixture<CardCardsMemberCreateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardCardsMemberCreateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardCardsMemberCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

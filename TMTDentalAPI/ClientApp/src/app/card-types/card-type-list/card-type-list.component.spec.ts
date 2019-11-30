import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardTypeListComponent } from './card-type-list.component';

describe('CardTypeListComponent', () => {
  let component: CardTypeListComponent;
  let fixture: ComponentFixture<CardTypeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardTypeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardTypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

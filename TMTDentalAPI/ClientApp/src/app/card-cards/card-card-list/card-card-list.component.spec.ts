import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardCardListComponent } from './card-card-list.component';

describe('CardCardListComponent', () => {
  let component: CardCardListComponent;
  let fixture: ComponentFixture<CardCardListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardCardListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardCardListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

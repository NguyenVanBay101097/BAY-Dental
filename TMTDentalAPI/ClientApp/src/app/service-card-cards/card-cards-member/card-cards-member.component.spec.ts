import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardCardsMemberComponent } from './card-cards-member.component';

describe('CardCardsMemberComponent', () => {
  let component: CardCardsMemberComponent;
  let fixture: ComponentFixture<CardCardsMemberComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardCardsMemberComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardCardsMemberComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

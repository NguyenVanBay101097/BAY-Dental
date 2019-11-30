import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardCardGridComponent } from './card-card-grid.component';

describe('CardCardGridComponent', () => {
  let component: CardCardGridComponent;
  let fixture: ComponentFixture<CardCardGridComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardCardGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardCardGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

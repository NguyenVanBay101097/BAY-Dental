import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardTypeCreateUpdateComponent } from './card-type-create-update.component';

describe('CardTypeCreateUpdateComponent', () => {
  let component: CardTypeCreateUpdateComponent;
  let fixture: ComponentFixture<CardTypeCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardTypeCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardTypeCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

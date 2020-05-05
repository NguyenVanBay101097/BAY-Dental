import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPluginTextareaComponent } from './facebook-plugin-textarea.component';

describe('FacebookPluginTextareaComponent', () => {
  let component: FacebookPluginTextareaComponent;
  let fixture: ComponentFixture<FacebookPluginTextareaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPluginTextareaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPluginTextareaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

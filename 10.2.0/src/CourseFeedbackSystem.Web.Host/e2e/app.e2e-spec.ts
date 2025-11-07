import { CourseFeedbackSystemTemplatePage } from './app.po';

describe('CourseFeedbackSystem App', function () {
    let page: CourseFeedbackSystemTemplatePage;

    beforeEach(() => {
        page = new CourseFeedbackSystemTemplatePage();
    });

    it('should display message saying app works', () => {
        page.navigateTo();
        expect(page.getParagraphText()).toEqual('app works!');
    });
});
